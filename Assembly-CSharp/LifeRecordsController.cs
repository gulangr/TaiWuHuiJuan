using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Config;
using FrameWork;
using GameData.Common;
using GameData.Domains.Character;
using GameData.Domains.Character.Display;
using GameData.Domains.Extra;
using GameData.Domains.LifeRecord;
using GameData.Domains.LifeRecord.GeneralRecord;
using GameData.GameDataBridge;
using GameData.Serializer;
using GameData.Utilities;
using UnityEngine;

// Token: 0x020001D3 RID: 467
public class LifeRecordsController : MonoBehaviour, ISingletonInit, IDisposable
{
	// Token: 0x06001E35 RID: 7733 RVA: 0x000D89F0 File Offset: 0x000D6BF0
	private void NotifyYearDataReady(int charId, LifeRecordsController.LifeRecordYearData yearData)
	{
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("CharacterId", charId);
		box.Set("Year", yearData.Year);
		box.SetObject("LifeRecordData", yearData);
		GEvent.OnEvent(UiEvents.OnCharacterLifeRecordYearReady, box);
	}

	// Token: 0x06001E36 RID: 7734 RVA: 0x000D8A40 File Offset: 0x000D6C40
	private void CheckWaitShowCacheForLifeRecordData()
	{
		bool flag = !this._curReadingRecordCharacter.BaseDataReady;
		if (!flag)
		{
			string keyStartString = string.Format("{0}_", this._curReadingRecordCharId);
			for (int i = this._waitShowingYearKeysCache.Count - 1; i >= 0; i--)
			{
				string key = this._waitShowingYearKeysCache[i];
				bool flag2 = key.StartsWith(keyStartString);
				if (flag2)
				{
					string yearString = key.Replace(keyStartString, string.Empty);
					int year;
					bool flag3 = int.TryParse(yearString, out year);
					if (flag3)
					{
						this.GetCharLifeRecordYearDataFromSvr(this._curReadingRecordCharacter, year);
					}
				}
			}
		}
	}

	// Token: 0x06001E37 RID: 7735 RVA: 0x000D8AE8 File Offset: 0x000D6CE8
	private void GetCharLifeRecordYearDataFromSvr(LifeRecordsController.LifeRecordCharacterData characterData, int year)
	{
		bool flag = characterData == null || !characterData.BaseDataReady;
		if (!flag)
		{
			bool flag2 = characterData.LifeRecordCache.ContainsKey(year);
			if (!flag2)
			{
				int yearStartDate = Mathf.Max(new int[]
				{
					(year - 1) * 12,
					characterData.BornDate,
					5
				});
				int yearEndDate = year * 12;
				int monthCount = yearEndDate - yearStartDate;
				bool flag3 = yearStartDate + monthCount > SingletonObject.getInstance<BasicGameData>().CurrDate;
				if (flag3)
				{
					monthCount = SingletonObject.getInstance<BasicGameData>().CurrDate - yearStartDate + 1;
				}
				bool flag4 = monthCount <= 0;
				if (!flag4)
				{
					LifeRecordsController.LifeRecordYearData yearData = new LifeRecordsController.LifeRecordYearData();
					yearData.Key = string.Format("{0}_{1}", characterData.CharId, year);
					yearData.Year = year;
					yearData.StartDate = yearStartDate;
					yearData.ArgCollection = new ArgumentCollection();
					characterData.LifeRecordCache.Add(year, yearData);
					LifeRecordDomainMethod.Call.GetByDate(this._gameDataListenerId, this._curReadingRecordCharId, yearData.StartDate, monthCount);
				}
			}
		}
	}

	// Token: 0x06001E38 RID: 7736 RVA: 0x000D8BEC File Offset: 0x000D6DEC
	private void SetCurrentCharacter(int charId, bool isDreamBack = false)
	{
		bool isNewCharacter = charId != this._curReadingRecordCharId;
		bool flag = isNewCharacter;
		if (flag)
		{
			this._waitShowingYearKeysCache.Clear();
		}
		this._curReadingRecordCharId = charId;
		this._curReadingRecordCharacter = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == charId);
		bool flag2 = this._curReadingRecordCharacter == null;
		if (flag2)
		{
			this._curReadingRecordCharacter = new LifeRecordsController.LifeRecordCharacterData
			{
				CharId = charId,
				BornDate = int.MinValue,
				LifeRecordCache = new Dictionary<int, LifeRecordsController.LifeRecordYearData>()
			};
			this._characterLifeRecordCache.Add(this._curReadingRecordCharacter);
			CharacterDomainMethod.Call.GetCharacterBirthDate(this._gameDataListenerId, this._curReadingRecordCharId);
			CharacterDomainMethod.Call.GetCharacterDisplayDataList(this._gameDataListenerId, new List<int>
			{
				this._curReadingRecordCharId
			});
		}
		else
		{
			bool flag3 = isNewCharacter && this._curReadingRecordCharacter.BaseDataReady;
			if (flag3)
			{
				ArgumentBox box = EasyPool.Get<ArgumentBox>();
				box.Set("CharacterId", charId);
				box.Set("BirthDate", this._curReadingRecordCharacter.BornDate);
				GEvent.OnEvent(UiEvents.OnGetCharBirthDateByLifeRecordModel, box);
			}
			if (isDreamBack)
			{
				ArgumentBox box2 = EasyPool.Get<ArgumentBox>();
				box2.Set("CharacterId", charId);
				box2.Set("BirthDate", this._curReadingRecordCharacter.BornDate);
				GEvent.OnEvent(UiEvents.OnGetCharBirthDateByLifeRecordModel, box2);
			}
		}
		bool flag4 = this._characterLifeRecordCache.Count > 10;
		if (flag4)
		{
			this._characterLifeRecordCache.RemoveAt(0);
		}
	}

	// Token: 0x06001E39 RID: 7737 RVA: 0x000D8DA0 File Offset: 0x000D6FA0
	public void StopReadRecords()
	{
		this._curReadingRecordCharacter = null;
		this._curReadingRecordCharId = -1;
	}

	// Token: 0x06001E3A RID: 7738 RVA: 0x000D8DB4 File Offset: 0x000D6FB4
	public void RemoveLatestYearRecordCache()
	{
		int curYear = SingletonObject.getInstance<TimeManager>().GetYear();
		foreach (LifeRecordsController.LifeRecordCharacterData characterData in this._characterLifeRecordCache)
		{
			characterData.LifeRecordCache.Remove(curYear);
		}
	}

	// Token: 0x06001E3B RID: 7739 RVA: 0x000D8E20 File Offset: 0x000D7020
	public void SetShowingYear(int year, int charId, bool isDreamBack = false)
	{
		bool flag = charId < 0;
		if (flag)
		{
			charId = this._curReadingRecordCharId;
		}
		bool flag2 = charId < 0;
		if (flag2)
		{
			throw new Exception("need available character id to handle show process");
		}
		this.SetCurrentCharacter(charId, isDreamBack);
		string key = string.Format("{0}_{1}", charId, year);
		LifeRecordsController.LifeRecordYearData yearData;
		bool flag3 = this._curReadingRecordCharacter.LifeRecordCache.TryGetValue(year, out yearData);
		if (flag3)
		{
			int yearStartDate = Mathf.Max(new int[]
			{
				(year - 1) * 12,
				this._curReadingRecordCharacter.BornDate,
				5
			});
			int yearEndDate = year * 12;
			int monthCount = yearEndDate - yearStartDate;
			bool flag4 = yearStartDate + monthCount > SingletonObject.getInstance<BasicGameData>().CurrDate;
			if (flag4)
			{
				monthCount = SingletonObject.getInstance<BasicGameData>().CurrDate - yearStartDate + 1;
			}
			bool flag5 = yearData.MonthRecordDataList != null && monthCount > yearData.MonthRecordDataList.Count;
			if (flag5)
			{
				yearData.RenderDataReady = false;
				this._curReadingRecordCharacter.LifeRecordCache.Remove(year);
				bool flag6 = !this._waitShowingYearKeysCache.Contains(key);
				if (flag6)
				{
					this._waitShowingYearKeysCache.Add(key);
				}
				this.GetCharLifeRecordYearDataFromSvr(this._curReadingRecordCharacter, year);
				return;
			}
			bool renderDataReady = yearData.RenderDataReady;
			if (renderDataReady)
			{
				this._waitShowingYearKeysCache.Remove(key);
				this.NotifyYearDataReady(charId, yearData);
				return;
			}
			bool flag7 = this._waitShowingYearKeysCache.Contains(key);
			if (flag7)
			{
				return;
			}
			bool flag8 = yearData.RenderInfoList != null;
			if (flag8)
			{
				this._charIdList.Clear();
				this._charIdList.AddRange(yearData.ArgCollection.Characters);
				this._charIdList.AddRange(yearData.ArgCollection.CharacterRealNames);
				RecordArgumentsRequest argRequest = new RecordArgumentsRequest(yearData.ArgCollection)
				{
					Characters = this._charIdList
				};
				LifeRecordDomainMethod.Call.GetRecordRenderInfoArguments(this._gameDataListenerId, yearData.Key, argRequest);
			}
		}
		else
		{
			bool flag9 = this._waitShowingYearKeysCache.Contains(key);
			if (flag9)
			{
				return;
			}
			bool baseDataReady = this._curReadingRecordCharacter.BaseDataReady;
			if (baseDataReady)
			{
				this.GetCharLifeRecordYearDataFromSvr(this._curReadingRecordCharacter, year);
			}
		}
		this._waitShowingYearKeysCache.Add(key);
	}

	// Token: 0x06001E3C RID: 7740 RVA: 0x000D9060 File Offset: 0x000D7260
	public void GetCharacterLifeRecordYearDataList(int charId, List<int> years, List<LifeRecordsController.LifeRecordYearData> retList)
	{
		LifeRecordsController.LifeRecordCharacterData characterData = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == charId);
		bool flag = characterData == null;
		if (!flag)
		{
			retList.Clear();
			for (int i = 0; i < years.Count; i++)
			{
				LifeRecordsController.LifeRecordYearData yearData;
				bool flag2 = characterData.LifeRecordCache.TryGetValue(years[i], out yearData) && yearData != null && yearData.RenderDataReady;
				if (flag2)
				{
					retList.Add(yearData);
				}
			}
			retList.Sort((LifeRecordsController.LifeRecordYearData left, LifeRecordsController.LifeRecordYearData right) => left.Year - right.Year);
		}
	}

	// Token: 0x06001E3D RID: 7741 RVA: 0x000D9118 File Offset: 0x000D7318
	public void GetDreamBackCharacterLifeRecordYearDataList(List<int> years, List<LifeRecordsController.LifeRecordYearData> retList)
	{
		retList.Clear();
		for (int i = 0; i < years.Count; i++)
		{
			LifeRecordsController.LifeRecordYearData yearData;
			bool flag = this._dreamBackReadingRecordCharacter.LifeRecordCache.TryGetValue(years[i], out yearData) && yearData != null && yearData.RenderDataReady;
			if (flag)
			{
				retList.Add(yearData);
			}
		}
		retList.Sort((LifeRecordsController.LifeRecordYearData left, LifeRecordsController.LifeRecordYearData right) => left.Year - right.Year);
	}

	// Token: 0x06001E3E RID: 7742 RVA: 0x000D91A0 File Offset: 0x000D73A0
	public LifeRecordsController.LifeRecordYearData GetCharacterLifeRecordYearData(int charId, int year)
	{
		LifeRecordsController.LifeRecordCharacterData characterData = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == charId);
		bool flag = characterData == null;
		LifeRecordsController.LifeRecordYearData result;
		if (flag)
		{
			result = null;
		}
		else
		{
			LifeRecordsController.LifeRecordYearData yearData;
			bool flag2 = characterData.LifeRecordCache.TryGetValue(year, out yearData) && yearData != null && yearData.RenderDataReady;
			if (flag2)
			{
				result = yearData;
			}
			else
			{
				result = null;
			}
		}
		return result;
	}

	// Token: 0x06001E3F RID: 7743 RVA: 0x000D9210 File Offset: 0x000D7410
	public short GetFavorabilityToTaiwuFromCacheData(int charId, bool dreamBack = false)
	{
		LifeRecordsController.LifeRecordCharacterData data = dreamBack ? this._dreamBackReadingRecordCharacter : this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == charId);
		bool flag = data != null;
		short result;
		if (flag)
		{
			result = data.DisplayData.FavorabilityToTaiwu;
		}
		else
		{
			result = short.MinValue;
		}
		return result;
	}

	// Token: 0x06001E40 RID: 7744 RVA: 0x000D926D File Offset: 0x000D746D
	public void ClearAllCache()
	{
		this._characterLifeRecordCache.Clear();
		this._curReadingRecordCharId = -1;
		this._renderedArgCollection.Clear();
		this._waitShowingYearKeysCache.Clear();
	}

	// Token: 0x06001E41 RID: 7745 RVA: 0x000D929C File Offset: 0x000D749C
	public void Dispose()
	{
		this._characterLifeRecordCache.Clear();
		GameDataBridge.AddDataUnMonitor(this._gameDataListenerId, 19, 70, ulong.MaxValue, uint.MaxValue);
		GameDataBridge.UnregisterListener(this._gameDataListenerId);
		this._gameDataListenerId = -1;
		this._curReadingRecordCharId = -1;
		this._renderedArgCollection.Clear();
		this._renderedArgCollection = null;
		this._waitShowingYearKeysCache.Clear();
		this._waitShowingYearKeysCache = null;
	}

	// Token: 0x06001E42 RID: 7746 RVA: 0x000D930C File Offset: 0x000D750C
	public void Init()
	{
		this._dreamBackRenderedArgCollection = new RenderedArgumentCollection();
		this._dispatcherInstance = DispatcherUtils.RegisterDispatcher();
		this._gameDataListenerId = GameDataBridge.RegisterListener(new GameDataBridge.NotificationHandler(this.OnNotifyGameData));
		this._renderedArgCollection = new RenderedArgumentCollection();
		this._characterLifeRecordCache = new List<LifeRecordsController.LifeRecordCharacterData>();
		this._waitShowingYearKeysCache = new List<string>();
		GameDataBridge.AddDataMonitor(this._gameDataListenerId, 19, 70, ulong.MaxValue, uint.MaxValue);
	}

	// Token: 0x06001E43 RID: 7747 RVA: 0x000D937B File Offset: 0x000D757B
	public void DreamBackInit()
	{
		ExtraDomainMethod.Call.GetAllDreamBackLifeRecords(this._gameDataListenerId);
	}

	// Token: 0x06001E44 RID: 7748 RVA: 0x000D938C File Offset: 0x000D758C
	public ValueTuple<int, int> GetDreamBackStartMonthAndEndMonthData()
	{
		return this._dreamBackStartAndEndMonth;
	}

	// Token: 0x06001E45 RID: 7749 RVA: 0x000D93A4 File Offset: 0x000D75A4
	public ValueTuple<int, int> GetDreamBackStartYearAndEndYearData()
	{
		int startYear = this._dreamBackStartAndEndMonth.Item1 / 12;
		int endYear = Mathf.Abs(this._dreamBackStartAndEndMonth.Item2) / 12;
		return new ValueTuple<int, int>(startYear, endYear);
	}

	// Token: 0x06001E46 RID: 7750 RVA: 0x000D93E0 File Offset: 0x000D75E0
	private void OnNotifyGameData(List<NotificationWrapper> notifications)
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
					this.FetchMethodReturnValue(notification.DomainId, notification.MethodId, notification.ValueOffset, wrapper.DataPool);
				}
			}
			else
			{
				this.HandleDataModification(notification.Uid, notification.ValueOffset, wrapper.DataPool);
			}
		}
	}

	// Token: 0x06001E47 RID: 7751 RVA: 0x000D9488 File Offset: 0x000D7688
	private void HandleDataModification(DataUid uid, int offset, RawDataPool dataPool)
	{
		ushort domainId = uid.DomainId;
		ushort num = domainId;
		if (num != 4)
		{
			if (num == 19)
			{
				bool flag = uid.DataId == 70;
				if (flag)
				{
					this._mirrorCharacters.Clear();
					Serializer.DeserializeModifications<int>(dataPool, offset, this._mirrorCharacters);
				}
			}
		}
	}

	// Token: 0x06001E48 RID: 7752 RVA: 0x000D94DC File Offset: 0x000D76DC
	private void ParseDreamBackReadingRecordCharacter(ReadonlyLifeRecordsWithDate data)
	{
		bool flag = data == null;
		if (!flag)
		{
			this._dreamBackReadingRecordCharacter = new LifeRecordsController.LifeRecordCharacterData
			{
				CharId = data.CharId,
				BornDate = data.StartDate,
				LifeRecordCache = new Dictionary<int, LifeRecordsController.LifeRecordYearData>()
			};
			CharacterDomainMethod.AsyncCall.GetCharacterDisplayData(this._dispatcherInstance, data.CharId, delegate(int offset, RawDataPool dataPool)
			{
				CharacterDisplayData characterDisplayData = new CharacterDisplayData();
				Serializer.Deserialize(dataPool, offset, ref characterDisplayData);
				this._dreamBackReadingRecordCharacter.DisplayData = characterDisplayData;
				this._dreamBackReadingRecordCharacter.Name = NameCenter.GetCharMonasticTitleOrNameByDisplayData(characterDisplayData, true, true);
			});
			ExtraDomainMethod.AsyncCall.GetDreamBackTaiwuBirthAndEndDates(this._dispatcherInstance, delegate(int offset, RawDataPool dataPool)
			{
				ValueTuple<int, int> valueTuple = new ValueTuple<int, int>(-1, -1);
				Serializer.Deserialize(dataPool, offset, ref valueTuple);
				this._dreamBackStartAndEndMonth = valueTuple;
				this.GetAllDreamBackYearData(this._dreamBackReadingRecordCharacter, data, valueTuple.Item1, valueTuple.Item2);
			});
		}
	}

	// Token: 0x06001E49 RID: 7753 RVA: 0x000D9580 File Offset: 0x000D7780
	private void GetAllDreamBackYearData(LifeRecordsController.LifeRecordCharacterData characterData, ReadonlyLifeRecordsWithDate data, int startMonth, int endMonth)
	{
		int startYear = SingletonObject.getInstance<TimeManager>().GetYearByDate(startMonth);
		int endYear = SingletonObject.getInstance<TimeManager>().GetYearByDate(endMonth);
		int monthCount = endMonth - startMonth;
		int fullYearCount = monthCount / 12;
		int extraMonths = monthCount % 12;
		List<ArgumentCollectionRenderArguments> argumentList = new List<ArgumentCollectionRenderArguments>();
		for (int i = startYear; i <= endYear; i++)
		{
			int yearStartDate = Mathf.Max(new int[]
			{
				(i - 1) * 12,
				characterData.BornDate,
				5
			});
			LifeRecordsController.LifeRecordYearData yearData = new LifeRecordsController.LifeRecordYearData
			{
				Key = string.Format("{0}_{1}", characterData.CharId, i),
				Year = i,
				StartDate = yearStartDate,
				ArgCollection = new ArgumentCollection(),
				RenderInfoList = new List<LifeRecordRenderInfo>()
			};
			ValueTuple<int, int[]> renderInfosOfDates = data.Records.GetRenderInfosOfDates(yearData.RenderInfoList, yearData.ArgCollection, yearData.StartDate, (i > fullYearCount) ? extraMonths : 12);
			int yearScore = renderInfosOfDates.Item1;
			int[] monthScoreArray = renderInfosOfDates.Item2;
			yearData.Score = yearScore;
			yearData.MonthRecordDataList = new List<LifeRecordsController.LifeRecordMonthData>();
			for (int j = 0; j < monthScoreArray.Length; j++)
			{
				int date = yearStartDate + j;
				LifeRecordsController.LifeRecordMonthData monthData = new LifeRecordsController.LifeRecordMonthData
				{
					Date = date,
					Month = (int)SingletonObject.getInstance<TimeManager>().GetMonthInYear(date),
					Score = monthScoreArray[j]
				};
				yearData.MonthRecordDataList.Add(monthData);
			}
			this._dreamBackCharIdList.Clear();
			this._dreamBackCharIdList.AddRange(yearData.ArgCollection.Characters);
			this._dreamBackCharIdList.AddRange(yearData.ArgCollection.CharacterRealNames);
			characterData.LifeRecordCache.Add(i, yearData);
			int currentIndex = i;
			RecordArgumentsRequest argRequest = new RecordArgumentsRequest(yearData.ArgCollection)
			{
				Characters = this._dreamBackCharIdList
			};
			LifeRecordDomainMethod.AsyncCall.GetRecordRenderInfoArguments(this._dispatcherInstance, yearData.Key, argRequest, true, delegate(int offset, RawDataPool dataPool)
			{
				ArgumentCollectionRenderArguments arguments = null;
				Serializer.Deserialize(dataPool, offset, ref arguments);
				argumentList.Add(arguments);
				bool flag = currentIndex == endYear;
				if (flag)
				{
					this.AssembleRenderData(argumentList, characterData);
				}
			});
		}
	}

	// Token: 0x06001E4A RID: 7754 RVA: 0x000D97E8 File Offset: 0x000D79E8
	private void AssembleRenderData(List<ArgumentCollectionRenderArguments> data, LifeRecordsController.LifeRecordCharacterData characterData)
	{
		foreach (ArgumentCollectionRenderArguments item in data)
		{
			string[] keyInfos = item.Key.Split('_', StringSplitOptions.None);
			int year;
			bool flag = int.TryParse(keyInfos[1], out year);
			if (flag)
			{
				LifeRecordsController.LifeRecordYearData yearData;
				bool flag2 = characterData.LifeRecordCache.TryGetValue(year, out yearData);
				if (flag2)
				{
					this._dreamBackRenderedArgCollection.Clear();
					GameMessageUtils.RenderFixedArguments(yearData.ArgCollection, this._dreamBackRenderedArgCollection, true);
					GameMessageUtils.RenderDynamicArguments(item, yearData.ArgCollection, this._dreamBackRenderedArgCollection, true, true);
					for (int i = 0; i < yearData.RenderInfoList.Count; i++)
					{
						LifeRecordRenderInfo info = yearData.RenderInfoList[i];
						LifeRecordItem config = LifeRecord.Instance.GetItem(info.RecordType);
						bool flag3 = config == null;
						if (!flag3)
						{
							LifeRecordsController.LifeRecordMonthData monthData = yearData.MonthRecordDataList.Find((LifeRecordsController.LifeRecordMonthData e) => e.Date == info.Date);
							bool flag4 = monthData != null;
							if (flag4)
							{
								int score = info.Score;
								string recordName = config.Name;
								string desc = info.GetText(this._dreamBackRenderedArgCollection);
								bool flag5 = monthData.RecordList == null;
								if (flag5)
								{
									monthData.RecordList = new List<ValueTuple<int, string, string, short>>();
								}
								monthData.RecordList.Add(new ValueTuple<int, string, string, short>(score, recordName, desc, info.RecordType));
							}
						}
					}
					yearData.MonthRecordDataList.Sort((LifeRecordsController.LifeRecordMonthData left, LifeRecordsController.LifeRecordMonthData right) => left.Date - right.Date);
					yearData.RenderDataReady = true;
				}
			}
		}
	}

	// Token: 0x06001E4B RID: 7755 RVA: 0x000D99F0 File Offset: 0x000D7BF0
	private bool IsAssembleRenderDataEnd()
	{
		return this._dreamBackReadingRecordCharacter.LifeRecordCache.Values.All((LifeRecordsController.LifeRecordYearData item) => item.RenderDataReady);
	}

	// Token: 0x06001E4C RID: 7756 RVA: 0x000D9A38 File Offset: 0x000D7C38
	public void SetDreamBackShowingYear(int year)
	{
		LifeRecordsController.LifeRecordYearData yearData;
		bool flag = this._dreamBackReadingRecordCharacter.LifeRecordCache.TryGetValue(year, out yearData);
		if (flag)
		{
			bool renderDataReady = yearData.RenderDataReady;
			if (renderDataReady)
			{
				this.NotifyYearDataReady(this._dreamBackReadingRecordCharacter.CharId, yearData);
			}
		}
	}

	// Token: 0x06001E4D RID: 7757 RVA: 0x000D9A80 File Offset: 0x000D7C80
	public void UpdateDreamBackYear()
	{
		ArgumentBox box = EasyPool.Get<ArgumentBox>();
		box.Set("CharacterId", this._dreamBackReadingRecordCharacter.CharId);
		box.Set("BirthDate", this._dreamBackReadingRecordCharacter.BornDate);
		GEvent.OnEvent(UiEvents.OnGetCharBirthDateByLifeRecordModel, box);
	}

	// Token: 0x06001E4E RID: 7758 RVA: 0x000D9AD0 File Offset: 0x000D7CD0
	private void FetchMethodReturnValue(ushort domainId, ushort methodId, int offset, RawDataPool dataPool)
	{
		if (domainId != 4)
		{
			if (domainId != 13)
			{
				if (domainId == 19)
				{
					if (methodId == 56)
					{
						ReadonlyLifeRecordsWithDate recordsWithDate = new ReadonlyLifeRecordsWithDate();
						Serializer.Deserialize(dataPool, offset, ref recordsWithDate);
						this.ParseDreamBackReadingRecordCharacter(recordsWithDate);
					}
				}
			}
			else if (methodId != 1)
			{
				if (methodId == 5)
				{
					ArgumentCollectionRenderArguments arguments = null;
					Serializer.Deserialize(dataPool, offset, ref arguments);
					string[] keyInfos = arguments.Key.Split('_', StringSplitOptions.None);
					int year;
					int charId;
					bool flag = int.TryParse(keyInfos[0], out charId) && int.TryParse(keyInfos[1], out year);
					if (flag)
					{
						LifeRecordsController.LifeRecordCharacterData characterData = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == charId);
						LifeRecordsController.LifeRecordYearData yearData;
						bool flag2 = characterData != null && characterData.LifeRecordCache.TryGetValue(year, out yearData);
						if (flag2)
						{
							this._renderedArgCollection.Clear();
							GameMessageUtils.RenderFixedArguments(yearData.ArgCollection, this._renderedArgCollection, true);
							GameMessageUtils.RenderDynamicArguments(arguments, yearData.ArgCollection, this._renderedArgCollection, true, true);
							for (int i = 0; i < yearData.RenderInfoList.Count; i++)
							{
								LifeRecordRenderInfo info = yearData.RenderInfoList[i];
								LifeRecordItem config = LifeRecord.Instance.GetItem(info.RecordType);
								bool flag3 = config == null;
								if (!flag3)
								{
									LifeRecordsController.LifeRecordMonthData monthData = yearData.MonthRecordDataList.Find((LifeRecordsController.LifeRecordMonthData e) => e.Date == info.Date);
									bool flag4 = monthData != null;
									if (flag4)
									{
										int score = info.Score;
										string recordName = config.Name;
										string desc = info.GetText(this._renderedArgCollection).ColorReplace();
										bool flag5 = monthData.RecordList == null;
										if (flag5)
										{
											monthData.RecordList = new List<ValueTuple<int, string, string, short>>();
										}
										monthData.RecordList.Add(new ValueTuple<int, string, string, short>(score, recordName, desc, info.RecordType));
									}
								}
							}
							bool flag6 = !this._mirrorCharacters.ContainsValue(characterData.CharId) && (yearData.StartDate <= characterData.BornDate || yearData.Year <= 1);
							if (flag6)
							{
								sbyte monthTemplateId = SingletonObject.getInstance<TimeManager>().GetMonthInYear(characterData.BornDate);
								MonthItem monthConfig = Month.Instance[monthTemplateId];
								string recordName2 = LocalStringManager.Get(LanguageKey.LK_Birth_Record_Name);
								LifeRecordsController.LifeRecordMonthData birthMonthData = yearData.MonthRecordDataList.Find((LifeRecordsController.LifeRecordMonthData e) => e.Date == characterData.BornDate);
								NameRelatedData nameRelatedData = new NameRelatedData();
								nameRelatedData.Gender = characterData.DisplayData.Gender;
								nameRelatedData.FullName = characterData.DisplayData.FullName;
								nameRelatedData.MonasticTitle = characterData.DisplayData.MonasticTitle;
								nameRelatedData.MonkType = characterData.DisplayData.MonkType;
								nameRelatedData.OrgGrade = characterData.DisplayData.OrgInfo.Grade;
								nameRelatedData.CharTemplateId = characterData.DisplayData.TemplateId;
								nameRelatedData.OrgTemplateId = characterData.DisplayData.OrgInfo.OrgTemplateId;
								string realName = NameCenter.GetRealName(ref nameRelatedData);
								bool flag7 = birthMonthData != null;
								if (flag7)
								{
									bool flag8 = birthMonthData.RecordList == null;
									if (flag8)
									{
										birthMonthData.RecordList = new List<ValueTuple<int, string, string, short>>();
									}
									string recordDesc = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Record, monthConfig.Name, realName);
									birthMonthData.RecordList.Insert(0, new ValueTuple<int, string, string, short>(50, recordName2, recordDesc, short.MinValue));
								}
								else
								{
									LifeRecordsController.LifeRecordMonthData firstMonthData = yearData.MonthRecordDataList.Find((LifeRecordsController.LifeRecordMonthData e) => e.Date == 5);
									bool flag9 = firstMonthData.RecordList == null;
									if (flag9)
									{
										firstMonthData.Score = 50;
										firstMonthData.RecordList = new List<ValueTuple<int, string, string, short>>();
									}
									bool flag10 = characterData.BornDate < 0;
									if (flag10)
									{
										int yearDiffer = SingletonObject.getInstance<TimeManager>().GetYearBetweenDate(characterData.BornDate, 0);
										string recordDesc2 = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Record_Before, yearDiffer, monthConfig.Name, realName);
										firstMonthData.RecordList.Insert(0, new ValueTuple<int, string, string, short>(50, recordName2, recordDesc2, short.MinValue));
									}
									else
									{
										string recordDesc3 = LocalStringManager.GetFormat(LanguageKey.LK_Birth_Record, monthConfig.Name, realName);
										firstMonthData.RecordList.Insert(0, new ValueTuple<int, string, string, short>(50, recordName2, recordDesc3, short.MinValue));
									}
								}
							}
							yearData.MonthRecordDataList.Sort((LifeRecordsController.LifeRecordMonthData left, LifeRecordsController.LifeRecordMonthData right) => left.Date - right.Date);
							yearData.RenderDataReady = true;
							this._waitShowingYearKeysCache.Remove(yearData.Key);
							this.NotifyYearDataReady(charId, yearData);
						}
					}
				}
			}
			else
			{
				ReadonlyLifeRecordsWithDate lifeRecordsWithDate = null;
				Serializer.Deserialize(dataPool, offset, ref lifeRecordsWithDate);
				LifeRecordsController.LifeRecordCharacterData characterData4 = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == lifeRecordsWithDate.CharId);
				bool flag11 = characterData4 != null;
				if (flag11)
				{
					LifeRecordsController.LifeRecordYearData yearData2 = characterData4.GetYearDataByStartDate(lifeRecordsWithDate.StartDate);
					bool flag12 = yearData2 != null;
					if (flag12)
					{
						yearData2.ArgCollection = new ArgumentCollection();
						yearData2.RenderInfoList = new List<LifeRecordRenderInfo>();
						ValueTuple<int, int[]> renderInfosOfDates = lifeRecordsWithDate.Records.GetRenderInfosOfDates(yearData2.RenderInfoList, yearData2.ArgCollection, yearData2.StartDate, lifeRecordsWithDate.MonthCount);
						int yearScore = renderInfosOfDates.Item1;
						int[] monthScoreArray = renderInfosOfDates.Item2;
						yearData2.Score = yearScore;
						yearData2.MonthRecordDataList = new List<LifeRecordsController.LifeRecordMonthData>();
						for (int j = 0; j < lifeRecordsWithDate.MonthCount; j++)
						{
							LifeRecordsController.LifeRecordMonthData monthData2 = new LifeRecordsController.LifeRecordMonthData();
							monthData2.Date = lifeRecordsWithDate.StartDate + j;
							monthData2.Month = (int)SingletonObject.getInstance<TimeManager>().GetMonthInYear(monthData2.Date);
							monthData2.Score = monthScoreArray[j];
							yearData2.MonthRecordDataList.Add(monthData2);
						}
						bool flag13 = this._waitShowingYearKeysCache.Contains(yearData2.Key);
						if (flag13)
						{
							this._charIdList.Clear();
							this._charIdList.AddRange(yearData2.ArgCollection.Characters);
							this._charIdList.AddRange(yearData2.ArgCollection.CharacterRealNames);
							RecordArgumentsRequest argRequest = new RecordArgumentsRequest(yearData2.ArgCollection)
							{
								Characters = this._charIdList
							};
							LifeRecordDomainMethod.Call.GetRecordRenderInfoArguments(this._gameDataListenerId, yearData2.Key, argRequest);
						}
					}
				}
			}
		}
		else if (methodId != 48)
		{
			if (methodId == 44)
			{
				int[] birthInfo = null;
				Serializer.Deserialize(dataPool, offset, ref birthInfo);
				bool flag14 = birthInfo == null || birthInfo[1] <= int.MinValue;
				if (!flag14)
				{
					LifeRecordsController.LifeRecordCharacterData characterData2 = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == birthInfo[0]);
					bool flag15 = characterData2 != null;
					if (flag15)
					{
						characterData2.BornDate = birthInfo[1];
						bool flag16 = birthInfo[0] == this._curReadingRecordCharId;
						if (flag16)
						{
							this.CheckWaitShowCacheForLifeRecordData();
						}
						ArgumentBox box = EasyPool.Get<ArgumentBox>();
						box.Set("CharacterId", birthInfo[0]);
						box.Set("BirthDate", birthInfo[1]);
						GEvent.OnEvent(UiEvents.OnGetCharBirthDateByLifeRecordModel, box);
					}
				}
			}
		}
		else
		{
			List<CharacterDisplayData> dataList = null;
			Serializer.Deserialize(dataPool, offset, ref dataList);
			int taiwuCharId = SingletonObject.getInstance<BasicGameData>().TaiwuCharId;
			using (List<CharacterDisplayData>.Enumerator enumerator = dataList.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					CharacterDisplayData data = enumerator.Current;
					LifeRecordsController.LifeRecordCharacterData characterData3 = this._characterLifeRecordCache.Find((LifeRecordsController.LifeRecordCharacterData e) => e.CharId == data.CharacterId);
					bool flag17 = characterData3 != null;
					if (flag17)
					{
						characterData3.DisplayData = data;
						characterData3.Name = NameCenter.GetCharMonasticTitleOrNameByDisplayData(data, data.CharacterId == taiwuCharId, true);
						bool flag18 = data.CharacterId == this._curReadingRecordCharId;
						if (flag18)
						{
							this.CheckWaitShowCacheForLifeRecordData();
						}
					}
				}
			}
		}
	}

	// Token: 0x040016EE RID: 5870
	public const string CharId = "CharacterId";

	// Token: 0x040016EF RID: 5871
	public const string Year = "Year";

	// Token: 0x040016F0 RID: 5872
	public const string Data = "LifeRecordData";

	// Token: 0x040016F1 RID: 5873
	public const string BirthDate = "BirthDate";

	// Token: 0x040016F2 RID: 5874
	private List<int> _charIdList = new List<int>();

	// Token: 0x040016F3 RID: 5875
	private List<int> _dreamBackCharIdList = new List<int>();

	// Token: 0x040016F4 RID: 5876
	private const short MaxCacheCharacterCount = 10;

	// Token: 0x040016F5 RID: 5877
	private int _gameDataListenerId;

	// Token: 0x040016F6 RID: 5878
	private List<LifeRecordsController.LifeRecordCharacterData> _characterLifeRecordCache;

	// Token: 0x040016F7 RID: 5879
	private List<string> _waitShowingYearKeysCache;

	// Token: 0x040016F8 RID: 5880
	private int _curReadingRecordCharId = -1;

	// Token: 0x040016F9 RID: 5881
	private ValueTuple<int, int> _dreamBackStartAndEndMonth;

	// Token: 0x040016FA RID: 5882
	private LifeRecordsController.LifeRecordCharacterData _curReadingRecordCharacter;

	// Token: 0x040016FB RID: 5883
	private LifeRecordsController.LifeRecordCharacterData _dreamBackReadingRecordCharacter;

	// Token: 0x040016FC RID: 5884
	private RenderedArgumentCollection _renderedArgCollection;

	// Token: 0x040016FD RID: 5885
	private RenderedArgumentCollection _dreamBackRenderedArgCollection;

	// Token: 0x040016FE RID: 5886
	private Dictionary<int, int> _mirrorCharacters = new Dictionary<int, int>();

	// Token: 0x040016FF RID: 5887
	private DispatcherInstance _dispatcherInstance;

	// Token: 0x0200142A RID: 5162
	public class LifeRecordMonthData
	{
		// Token: 0x0400A014 RID: 40980
		public int Date;

		// Token: 0x0400A015 RID: 40981
		public int Month;

		// Token: 0x0400A016 RID: 40982
		public int Score;

		// Token: 0x0400A017 RID: 40983
		[TupleElementNames(new string[]
		{
			"score",
			"name",
			"desc",
			"type"
		})]
		public List<ValueTuple<int, string, string, short>> RecordList;
	}

	// Token: 0x0200142B RID: 5163
	public class LifeRecordYearData
	{
		// Token: 0x0400A018 RID: 40984
		public string Key;

		// Token: 0x0400A019 RID: 40985
		public int StartDate;

		// Token: 0x0400A01A RID: 40986
		public ArgumentCollection ArgCollection;

		// Token: 0x0400A01B RID: 40987
		public List<LifeRecordRenderInfo> RenderInfoList;

		// Token: 0x0400A01C RID: 40988
		public int Year;

		// Token: 0x0400A01D RID: 40989
		public int Score;

		// Token: 0x0400A01E RID: 40990
		public bool RenderDataReady;

		// Token: 0x0400A01F RID: 40991
		public List<LifeRecordsController.LifeRecordMonthData> MonthRecordDataList;
	}

	// Token: 0x0200142C RID: 5164
	private class LifeRecordCharacterData
	{
		// Token: 0x17001659 RID: 5721
		// (get) Token: 0x0600CB12 RID: 51986 RVA: 0x0059327A File Offset: 0x0059147A
		public bool BaseDataReady
		{
			get
			{
				return this.BornDate > int.MinValue && !string.IsNullOrEmpty(this.Name);
			}
		}

		// Token: 0x1700165A RID: 5722
		// (get) Token: 0x0600CB13 RID: 51987 RVA: 0x0059329A File Offset: 0x0059149A
		// (set) Token: 0x0600CB14 RID: 51988 RVA: 0x005932A2 File Offset: 0x005914A2
		public bool DataReady { get; private set; }

		// Token: 0x0600CB15 RID: 51989 RVA: 0x005932AC File Offset: 0x005914AC
		public LifeRecordsController.LifeRecordYearData GetYearDataByStartDate(int startDate)
		{
			foreach (KeyValuePair<int, LifeRecordsController.LifeRecordYearData> pair in this.LifeRecordCache)
			{
				bool flag = pair.Value.StartDate == startDate;
				if (flag)
				{
					return pair.Value;
				}
			}
			return null;
		}

		// Token: 0x0400A020 RID: 40992
		public int CharId;

		// Token: 0x0400A021 RID: 40993
		public int BornDate;

		// Token: 0x0400A022 RID: 40994
		public CharacterDisplayData DisplayData;

		// Token: 0x0400A023 RID: 40995
		public string Name;

		// Token: 0x0400A025 RID: 40997
		public Dictionary<int, LifeRecordsController.LifeRecordYearData> LifeRecordCache;
	}
}
